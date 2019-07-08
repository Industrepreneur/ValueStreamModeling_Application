import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { hot } from 'react-hot-loader'

// const App = () => <div>Hello World!</div>
import { App } from './App'

const HotApp = hot(module)(App)

const render: any = Component => {
  ReactDOM.render(<HotApp /> as any, document.getElementById('react-root'))
}
render(HotApp)
