import * as React from 'react'
import * as ReactDOM from 'react-dom'
// import { hot } from 'react-hot-loader'

// const App = () => <div>Hello World!</div>
// import { App } from './App'

// const HotApp = hot(module)(App)

// const render: any = Component => {
//   ReactDOM.render(<HotApp /> as any, document.getElementById('react-root'))
// }
// render(HotApp)

import { AppContainer } from 'react-hot-loader'
import { App } from './App'

const render = (Component) => {
  ReactDOM.render(
    <AppContainer>
      <Component />
    </AppContainer>,
    document.getElementById('react-root'),
  )
}

render(App)

// Webpack Hot Module Replacement API
declare var module: any
if (module.hot) {
  module.hot.accept('./App', () => { render(App) })
}
