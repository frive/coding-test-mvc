/*global require*/
(function () {
  var fs = require('fs');
  var path = require('path');
  // include gulp
  var gulp = require('gulp');
  // include plug-ins
  var del = require('del');
  var uglify = require('gulp-uglify');
  var newer = require('gulp-newer');
  var useref = require('gulp-useref');
  var gulpif = require('gulp-if');
  var minifyCss = require('gulp-clean-css');
  var gulpReplace = require('gulp-replace');
  var htmlBuild = require('gulp-htmlbuild');
  var eventStream = require('event-stream');
  var runSequence = require('run-sequence');
  var argv = require('yargs').argv;
  var nugetRestore = require('gulp-nuget-restore');
  var msbuild = require('gulp-msbuild');
  var msdeploy = require('gulp-msdeploy');

  var webRoot = 'wwwroot/';
  var webBuildDir = argv.serviceStackSettingsDir || './wwwroot_build/';
  var configFile = 'config.json';
  var configDir = webBuildDir + 'publish/';
  var configPath = configDir + configFile;
  var appSettingsFile = 'appsettings.txt';
  var appSettingsDir = webBuildDir + 'deploy/';
  var appSettingsPath = appSettingsDir + appSettingsFile;

  function createConfigsIfMissing() {
    if (!fs.existsSync(configPath)) {
      if (!fs.existsSync(configDir)) {
        fs.mkdirSync(configDir);
      }
      fs.writeFileSync(configPath, JSON.stringify({
        "iisApp": "CodingTestMVC",
        "serverAddress": "deploy-server.example.com",
        "userName": "{WebDeployUserName}",
        "password": "{WebDeployPassword}"
      }, null, 4));
    }
    if (!fs.existsSync(appSettingsPath)) {
      if (!fs.existsSync(appSettingsDir)) {
        fs.mkdirSync(appSettingsDir);
      }
      fs.writeFileSync(appSettingsPath,
          '# Release App Settings\r\nDebugMode false');
    }
  }

  // Deployment config
  createConfigsIfMissing();
  var config = require(configPath);

  // htmlbuild replace for CDN references
  function pipeTemplate(block, template) {
    eventStream.readArray([
        template
    ].map(function (str) {
      return block.indent + str;
    })).pipe(block);
  }

  // Tasks

  gulp.task('www-clean-dlls', function (done) {
    var binPath = webRoot + '/bin/';
    del(binPath, done);
  });
  gulp.task('www-copy-bin', function () {
    var binDest = webRoot + 'bin/';
    return gulp.src('./bin/**/*')
        .pipe(newer(binDest))
        .pipe(gulp.dest(binDest));
  });
  gulp.task('www-copy-appdata', function () {
    return gulp.src('./App_Data/**/*')
        .pipe(newer(webRoot + 'App_Data/'))
        .pipe(gulp.dest(webRoot + 'App_Data/'));
  });
  gulp.task('www-copy-webconfig', function () {
    return gulp.src('./web.config')
        .pipe(newer(webRoot))
        .pipe(gulpReplace('<compilation debug="true" targetFramework="4.5">', '<compilation targetFramework="4.5">'))
        .pipe(gulp.dest(webRoot));
  });
  gulp.task('www-copy-asax', function () {
    return gulp.src('./Global.asax')
        .pipe(newer(webRoot))
        .pipe(gulp.dest(webRoot));
  });
  gulp.task('www-clean-client-assets', function (done) {
    del([
        webRoot + '**/*.*',
        '!wwwroot/bin/**/*.*', //Don't delete dlls
        '!wwwroot/App_Data/**/*.*', //Don't delete App_Data
        '!wwwroot/**/*.asax', //Don't delete asax
        '!wwwroot/**/*.config', //Don't delete config
        '!wwwroot/appsettings.txt' //Don't delete deploy settings
    ], done);
  });
  gulp.task('www-copy-partials', function () {
    var partialsDest = webRoot + 'partials';
    return gulp.src('partials/**/*.html')
        .pipe(newer(partialsDest))
        .pipe(gulp.dest(partialsDest));
  });
  gulp.task('www-copy-fonts', function () {
    return gulp.src('./node_modules/bootstrap@3.2.0/fonts/*.*')
        .pipe(gulp.dest(webRoot + 'lib/fonts/'));
  });
  gulp.task('www-copy-images', function () {
    return gulp.src('./img/**/*')
        .pipe(gulp.dest(webRoot + 'img/'));
  });
  gulp.task('www-bundle-html', function () {
    return gulp.src(['./default.cshtml', './default.html'])
        .pipe(useref())
        .pipe(gulpif('*.js', uglify()))
        .pipe(gulpif('*.css', minifyCss()))
        .pipe(htmlBuild({
          bootstrapCss: function (block) {
            pipeTemplate(block, '<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.2.0/css/bootstrap.min.css" />');
          }
        }))
        .pipe(gulp.dest(webRoot));
  });
  gulp.task('www-copy-deploy-files', function () {
    return gulp.src(webBuildDir + 'deploy/*.*')
        .pipe(newer(webRoot))
        .pipe(gulp.dest(webRoot));
  });
  gulp.task('www-msbuild', function () {
    return gulp.src('../../CodingTestMVC.sln')
        .pipe(nugetRestore())
        .pipe(msbuild({
          targets: ['Clean', 'Build'],
          stdout: true,
          verbosity: 'quiet'
        }
        ));
  });

  gulp.task('01-package-server', function (callback) {
    runSequence('www-msbuild', 'www-clean-dlls',
            [
                'www-copy-bin',
                'www-copy-appdata',
                'www-copy-webconfig',
                'www-copy-asax',
                'www-copy-deploy-files'
            ],
            callback);
  });

  gulp.task('02-package-client', function (callback) {
    runSequence('www-clean-client-assets',
            [
                'www-copy-fonts',
                'www-copy-partials',
                'www-copy-images',
                'www-bundle-html'
            ],
            callback);
  });

  gulp.task('www-msdeploy-pack', function () {
    return gulp.src('wwwroot/')
        .pipe(msdeploy({
          verb: 'sync',
          sourceType: 'iisApp',
          dest: {
            'package': path.resolve('./webdeploy.zip')
          }
        }));
  });

  gulp.task('www-msdeploy-push', function () {
    return gulp.src('./webdeploy.zip')
        .pipe(msdeploy({
          verb: 'sync',
          allowUntrusted: 'true',
          sourceType: 'package',
          dest: {
            iisApp: config.iisApp,
            wmsvc: config.serverAddress,
            UserName: config.userName,
            Password: config.password
          }
        }));
  });

  gulp.task('03-deploy-app', function (callback) {
    runSequence('www-msdeploy-pack', 'www-msdeploy-push',
        callback);
  });

  gulp.task('package-and-deploy', function (callback) {
    runSequence('01-package-server', '02-package-client', '03-deploy-app',
        callback);
  });
  gulp.task('default', function (callback) {
    runSequence('01-package-server', '02-package-client',
            callback);
  });
})();