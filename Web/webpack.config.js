const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
 
module.exports = {
    entry: {
        site: "./wwwroot/js/site.js",
        index: "./Views/Home/Index.cshtml.js",
        book: "./Views/Book/Book.cshtml.js",
        store: "./Views/Store/Store.cshtml.js",
    },
    output: {
        filename: "[name].entry.js",
        path: path.resolve(__dirname, "wwwroot", "dist"),
        clean: true
    },
    devtool: "source-map",
    mode: "development",
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [{ loader: MiniCssExtractPlugin.loader }, "css-loader"]
            },
            {
                test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
                type: "asset"
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
                type: "asset"
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        })
    ]
};