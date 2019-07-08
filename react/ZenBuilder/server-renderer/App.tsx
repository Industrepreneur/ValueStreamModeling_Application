import * as React from 'react'
import Button from 'material-ui/Button'

export class App extends React.Component {
  // Remove the server-side injected CSS.
  componentDidMount() {
    const jssStyles = document.getElementById('jss-server-side')
    if (jssStyles && jssStyles.parentNode) {
      jssStyles.parentNode.removeChild(jssStyles)
    }
  }

  render() {
    return (
      <div>
        Hello universal rendering!
        <br />
        <Button>Material UI v1 Button</Button>
      </div>
    )
  }
}
