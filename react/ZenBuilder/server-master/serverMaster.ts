import * as express from 'express'

let port = 3000
let title = 'Master'
process.title = `${port}:${title}`

const app = express()

app.get('/', (req, res) => {
  res.send('Hello World!')
})

app.get('/ping', (req, res) => {
  res.send('pong')
})

app.listen(port, () => {
  console.log(`Master server listening on port ${port}!`)
})
