const path = require('path');

module.exports = {
    mode: 'development',
    entry: {
        'gamepush-unity': './src/index.js',
    },
    devtool: 'cheap-source-map',
    output: {
        path: path.resolve(__dirname, 'dist/TemplateData'),
        filename: 'gamepush-unity.js',
        crossOriginLoading: 'anonymous',
    },
    watchOptions: {
        ignored: [
            'node_modules/**',
            'Packages/**',
            'Logs/**',
            'UserSettings/**',
        ],
    },
    devServer: {
        writeToDisk: true,
        contentBase: path.join(__dirname, 'dist'),
        compress: true,
        port: 8888,
        hot: true,
        https: true,
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Headers': 'Origin, X-Requested-With, Content-Type, Accept',
        },
    },
};
