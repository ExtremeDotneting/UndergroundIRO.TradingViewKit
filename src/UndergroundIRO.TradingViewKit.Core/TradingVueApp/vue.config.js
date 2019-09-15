const merge = require('webpack-merge')

function findHashAndRemove(obj) {
    function funcInternal(obj, prefix, stackLevel) {
        for (var propName in obj) {
            var value = obj[propName];
            if (typeof value === 'string') {
                if (value.indexOf('hash:8') > 0) {
                    console.log('\n' + prefix + propName + ' = ' + value + ' ;');
                    value = value
                        .replace('.[hash:8]', '')
                        .replace('.[contenthash:8]', '');
                    obj[propName] = value;
                    console.log(prefix + propName + ' = ' + value + ' ;');
                }
            }
            else {
                try {
                    if (stackLevel > 3)
                        return;
                    funcInternal(value, propName + '.', stackLevel + 1);
                } catch (e) { }
            }
        }
    }
    funcInternal(obj, '', 1);
}

module.exports = {
    //To make it work in file system (without server).
    publicPath:'./',

    configureWebpack: config => {
        findHashAndRemove(config);
        //console.log("PUBLIC PATH: " + config.publicPath)
    },

    chainWebpack: config => {
        config.module.rule('images').use('url-loader')
            .tap(options => {
                options.fallback.options.name = 'img/[name].[ext]';
                return options;
            });
        config.module.rule('svg').use('file-loader')
            .tap(options => {
                options.name = 'img/[name].[ext]';
                return options;
            });
        config.module.rule('media').use('url-loader')
            .tap(options => {
                options.fallback.options.name = 'media/[name].[ext]';
                return options;
            });
        config.module.rule('fonts').use('url-loader')
            .tap(options => {
                options.fallback.options.name = 'fonts/[name].[ext]';
                return options;
            });
    }
}