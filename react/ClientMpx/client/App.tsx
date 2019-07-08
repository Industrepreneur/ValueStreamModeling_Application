import * as React from 'react'

import { Routes } from './Routes'

import { MuiThemeProvider, createMuiTheme } from 'material-ui/styles'
// Colors from Value Stream Modeling
const theme = createMuiTheme({
  palette: {
    primary: { main: '#3E93D8' },
    secondary: { main: '#6DB3F2' },
  },
})

export class App extends React.Component<{}, {}> {
  render() {
    return (
      <div>
        <MuiThemeProvider theme={theme}>
          <Routes />
        </MuiThemeProvider>
      </div>
    )
  }
}
