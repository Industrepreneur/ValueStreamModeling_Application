import * as webpackConfigBuilder from '../ZenBuilder/webpackConfigBuilder'
import * as path from 'path'
const config = webpackConfigBuilder.createConfig({
  dirName: __dirname,
  outputPath: path.join(__dirname + './../../mpx/react'),
  isProduction: true,
})
export default config