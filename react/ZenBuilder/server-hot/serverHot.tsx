import * as _ from 'lodash'
import * as express from 'express'
import * as path from 'path'
import * as serveIndex from 'serve-index'

import * as webpack from 'webpack'
import * as webpackDevMiddleware from 'webpack-dev-middleware'
import * as webpackHotMiddleware from 'webpack-hot-middleware'

import * as history from 'connect-history-api-fallback'
import * as proxy from 'http-proxy-middleware'
// import * as webpackConfig from './../webpack.config.js'

export interface IProxy {
  in: string
  out: string
}
export function createServer(options: {
  dirName: string
  port?: number
  title?: string
  webpackConfig: any
  proxy?: IProxy[]
}) {
  _.defaults(options, {
    port: 3003,
    title: 'Hot 2',
    proxy: [],
  })
  process.title = `${options.port}:${options.title}`

  const app = express()

  const compiler = webpack(options.webpackConfig)

  _.forEach(options.proxy, c => {
    console.log(`proxy ${c.in} to ${c.out}`)
    app.use(c.in, proxy({ target: c.out, changeOrigin: true }))
  })

  console.log('/public is static')
  let publicPath = path.join(options.dirName, '/public')
  app.use(
    '/public',
    express.static(publicPath),
    serveIndex(publicPath, { icons: true })
  )

  // // Pass-thru for all routing
  app.use(history())

  // Dev and hot middleware
  app.use(
    webpackDevMiddleware(compiler, {
      // webpack-dev-middleware options
      noInfo: true,
      logLevel: 'warn',
      publicPath: options.webpackConfig.output.publicPath,
      historyApiFallback: true,
    })
  )

  app.use(webpackHotMiddleware(compiler))

  app.get('/', (req, res) => {
    res.send('Hello World!')
  })

  app.get('/ping', (req, res) => {
    res.send('pong')
  })

  app.listen(options.port, () => {
    console.log(
      `Hot server ${options.title} listening on port ${options.port}!`
    )
  })
}
