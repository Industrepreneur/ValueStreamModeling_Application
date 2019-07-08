const path = require('path')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const CleanWebpackPlugin = require('clean-webpack-plugin')
const webpack = require('webpack')

const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin')

const config = {
  entry: ['./client/AppIndex.tsx'],
  output: {
    filename: 'bundle.js',
    //path: __dirname + "/dist",
    path: __dirname + '/../../dev-larson/mpx/react',
    publicPath: '/',
  },

  // Enable sourcemaps for debugging webpack's output.
  devtool: 'source-map',
  mode: 'production',
  // devtool: "eval",

  plugins: [
    // new CleanWebpackPlugin(['dist']),
    new webpack.NamedModulesPlugin(),
  ],

  resolve: {
    extensions: ['.js', '.ts', '.tsx', '.json'],

    plugins: [
      new TsconfigPathsPlugin({
        configFile: './tsconfig.json',
        logLevel: 'info',
        extensions: ['.ts', '.tsx'],
      }),
    ],
  },

  module: {
    rules: [
      // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
      {
        test: /\.tsx?$/,
        loaders: ['ts-loader'],
      },

      // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
      { enforce: 'pre', test: /\.js$/, loader: 'source-map-loader' },
    ],
  },

  // When importing a module whose path matches one of the following, just
  // assume a corresponding global variable exists and use that instead.
  // This is important because it allows us to avoid bundling all of our
  // dependencies, which allows browsers to cache those libraries between builds.
  externals: {
    //react: "React",
    //"react-dom": "ReactDOM"
  },
}
export default config
export { config }