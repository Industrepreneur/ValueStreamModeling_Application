import * as React from 'react'

import Button from 'material-ui/Button'
import { FormGroup, FormControlLabel } from 'material-ui/Form'
import Switch from 'material-ui/Switch'
import Dialog, {
  DialogActions,
  DialogContent,
  DialogTitle,
} from 'material-ui/Dialog'

export class DialogDeleteConfirmation extends React.Component<{
  isOpen: boolean,
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
        <DialogTitle id="confirmation-dialog-title">Are you sure you want to delete these rows?</DialogTitle>
        {/* <DialogContent>
        content goes here
        </DialogContent> */}
        <DialogActions>
          <Button onClick={this._onCancel} color="primary">
            Cancel
          </Button>
          <Button onClick={this._onOk} color="primary">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    )
  }
}
