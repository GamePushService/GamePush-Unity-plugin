const path = require('path');

module.exports = {
    mode: 'production',
    entry: {
        'gamepush-unity': './src/index.js',
    },
    devtool: 'source-map',
    output: {
        path: path.resolve(__dirname, '../Demo/Assets/WebGLTemplates/GamePush/TemplateData'),
        filename: 'gamepush-unity.js',
        crossOriginLoading: 'anonymous',
    },
};
