"use strict";

const gulp = require("gulp"),
    babel = require('gulp-babel'),
    concat = require("gulp-concat"),
    cssmin = require("gulp-clean-css"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

const regex = {
    css: /\.css$/,
    js: /\.js$/
};

gulp.task("bundle:js", function () {
    const tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle:css", function () {
    const tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:js", function () {
    const tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(babel({
                presets: ['@babel/env']
            }))
            .pipe(concat(bundle.outputFileName.replace(regex.js, ".min.js")))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:css", function () {
    const tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName.replace(regex.css, ".min.css")))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("bundle", gulp.parallel("bundle:js", "bundle:css"));
gulp.task("min", gulp.parallel("min:js", "min:css"));

gulp.task("clean", function () {
    const files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });

    return del(files);
});

gulp.task("watch", function () {
    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:css"]);
    });
});

gulp.task("build", gulp.parallel("bundle", "min"));
gulp.task("default", gulp.series("clean", "build"));

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}