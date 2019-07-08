import * as express from 'express'

let port = 3001
let title = 'Api'
process.title = `${port}:${title}`

const app = express()

app.get('/', (req, res) => {
  res.send('API Server V1.0.0')
})

app.get('/ping', (req, res) => {
  res.send('pong')
})

app.get('/ping', (req, res) => {
  res.send('pong')
})

app.listen(port, () => {
  console.log(`API Server listening on port ${port}!`)
})
