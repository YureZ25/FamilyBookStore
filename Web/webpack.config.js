const path = require('path');

module.exports = {
    entry: {
        site: './wwwroot/js/site.js'
    },
    output: {
        filename: '[name].entry.js',
        path: path.resolve(__dirname, 'wwwroot', 'dist'),
        clean: true
    },
    devtool: 'source-map',
    mode: 'development',
    module: {
        rules: [
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
                type: 'asset'
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
                type: 'asset'
            }
        ]
    }
};