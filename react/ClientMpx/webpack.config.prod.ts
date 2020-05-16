import * as webpackConfigBuilder from '../zen-builder/webpackConfigBuilder'
import * as path from 'path'
const config = webpackConfigBuilder.createConfig({
  dirName: __dirname,
  outputPath: path.join(__dirname + './../../mpx/react'),
  isProduction: true,
  isHot: false,
})
export default config
