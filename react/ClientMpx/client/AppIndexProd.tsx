import * as React from 'react'
import * as ReactDOM from 'react-dom'

// const App = () => <div>Hello World!</div>
import { App } from './App'

const render: any = (Component) => {
  ReactDOM.render(<App /> as any, document.getElementById('react-root'))
}
render(App)
