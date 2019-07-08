import * as webpackConfigBuilder from '../ZenBuilder/webpackConfigBuilder'
import * as path from 'path'
const config = webpackConfigBuilder.createConfig({
  dirName: __dirname, 
  // outputPath: 'C:/dev-larson/mpx/react/', 
  outputPath: path.join(__dirname + './../../mpx/react'),
  isProduction: false,
})
export default config