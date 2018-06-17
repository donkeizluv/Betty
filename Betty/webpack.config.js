'use strict';
const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin')
const { VueLoaderPlugin } = require('vue-loader')
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const dev = process.env.NODE_ENV == 'development';
module.exports = {
    entry: {
        Home: './wwwroot/src/Home/app.js'
    },
    output: {
        path: __dirname + "/wwwroot/dist/",
        filename: dev? 'app.js' : "app_[chunkhash].js"
        // publicPath: '/dist/',
        // chunkFilename: '[name].chunk.js',
        // chunkFilename: '[chunkhash].chunk.js'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader?cacheDirectory'
                }
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader'
                  ]
                
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.(svg|eot|woff|ttf|svg|woff2)$/,
                loader: "file-loader",
                options: {
                    outputPath: 'assets/',
                    publicPath: '/dist/assets',
                    name: '[name].[ext]'
                }
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin(['wwwroot/dist/*'], []),
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin({
            filename: dev? 'app.css' : 'app_[chunkhash].css'
        })
    ],
    stats: {
        warnings: false
    }
};