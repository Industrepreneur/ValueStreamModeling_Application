import * as express from 'express'
import * as path from 'path'
import * as React from 'react'

import { renderToString } from 'react-dom/server'
import { SheetsRegistry } from 'react-jss/lib/jss'
import JssProvider from 'react-jss/lib/JssProvider'
import {
  MuiThemeProvider,
  createMuiTheme,
  createGenerateClassName,
} from 'material-ui/styles'
import { green, red } from 'material-ui/colors'

import { App } from './App'

let port = 3002
let title = 'Renderer'
process.title = `${port}:${title}`

const app = express()

function handleRender(req, res) {
  // Create a sheetsRegistry instance.
  const sheetsRegistry = new SheetsRegistry()

  // Create a theme instance.
  const theme = createMuiTheme({
    palette: {
      primary: green,
      accent: red,
      type: 'light',
    },
  })

  const generateClassName = createGenerateClassName()

  // Render the component to a string.
  const html = renderToString(
    <JssProvider
      registry={sheetsRegistry}
      generateClassName={generateClassName}
    >
      <MuiThemeProvider theme={theme} sheetsManager={new Map()}>
        <App />
      </MuiThemeProvider>
    </JssProvider>,
  )

  // Grab the CSS from our sheetsRegistry.
  const css = sheetsRegistry.toString()

  // Send the rendered page back to the client.
  res.send(renderFullPage(html, css))
}

function renderFullPage(html, css) {
  return `
    <!doctype html>
    <html>
      <head>
        <title>Material-UI</title>
        <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500">
      </head>
      <body>
        <div id="react-root">${html}</div>
        <style id="jss-server-side">${css}</style>
      </body>
    </html>
  `
}

app.use(handleRender)

app.get('/', (req, res) => {
  res.send('Hello World!')
})

app.get('/ping', (req, res) => {
  res.send('pong')
})

app.listen(port, () => {
  console.log(`Universal renderer server listening on port ${port}!`)
})
