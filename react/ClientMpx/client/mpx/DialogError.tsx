import * as React from 'react'

import Button from 'material-ui/Button'
import { FormGroup, FormControlLabel } from 'material-ui/Form'
import Switch from 'material-ui/Switch'
import Dialog, {
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from 'material-ui/Dialog'

export class DialogError extends React.Component<{
  isOpen: boolean,
  title?: string
  error: string,
  onClose: (isOk: boolean) => void
}> {
  state = {}

  _onCancel = () => {
    this.props.onClose(false)
  }

  _onOk = () => {
    this.props.onClose(true)
  }

  render() {
    return (
      <Dialog maxWidth="xs" open={this.props.isOpen} onClose={this._onCancel}>
        <DialogTitle id="error-dialog-title">{this.props.title || 'Error'}</DialogTitle>
        <DialogContent>
          <DialogContentText id="error-dialog-description">
            {this.props.error}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={this._onOk} color="primary">
            Ok
          </Button>
        </DialogActions>
      </Dialog>
    )
  }
}
