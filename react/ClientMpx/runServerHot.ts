import * as serverHot from '../ZenBuilder/server-hot/serverHot'
import webpackConfig from './webpack.config.dev'
serverHot.createServer({
  dirName: __dirname,
  port: 3007,
  title: 'MPX',
  webpackConfig,
  proxy: [{ in: '/api/mpx/v1/*', out: 'http://localhost:61752/' }],
})
