import * as React from 'react'

import Button from 'material-ui/Button'
import { FormGroup, FormControlLabel } from 'material-ui/Form'
import Switch from 'material-ui/Switch'

import { FaIcon } from '@zen-components/common/FaIcon'

import { DialogDeleteConfirmation } from './DialogDeleteConfirmation'

export class GridTitleBar extends React.Component<{
  title: string
  showAdvanced: boolean
  onToggleShowAdvanced: () => void
  onBack?: () => void,
  onAdd: () => void
  onDeleteSelected: () => void
  hasSelected: boolean
}> {
  state = {
    isDialogDeleteConfirmationOpen: false,
  }

  _onDeleteSelected = () => {
    this.setState({ isDialogDeleteConfirmationOpen: true })
  }
  _onClose_DialogDeleteConfirmation = isOk => {
    this.setState({ isDialogDeleteConfirmationOpen: false })
    if (isOk && this.props.onDeleteSelected) {
      this.props.onDeleteSelected()
    }
  }

  render() {
    let { title } = this.props

    return (
      <div style={{ padding: '10px' }}>
        <div
          style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            fontFamily: 'Roboto',
            fontWeight: 'bold',
          }}
        >
          <div>{title}</div>
          <div>
            <FormControlLabel
              control={
                <Switch
                  checked={this.props.showAdvanced}
                  onChange={this.props.onToggleShowAdvanced}
                  value="showAdvanced"
                  color="primary"
                />
              }
              label="Show Advanced"
            />
          </div>
        </div>
        <div>

          {
            this.props.onBack && (
              <Button variant="flat" color="primary" onClick={this.props.onBack}>
                <FaIcon icon="chevron-left" />
                &nbsp; Back
          </Button>
            )
          }

          <Button variant="flat" color="primary" onClick={this.props.onAdd}>
            <FaIcon icon="plus" />
            &nbsp; Add
          </Button>

          <Button
            variant="flat"
            color="primary"
            disabled={!this.props.hasSelected}
            onClick={this._onDeleteSelected}
          >
            <FaIcon icon="trash" />
            &nbsp; Delete Selected
          </Button>
        </div>

        <DialogDeleteConfirmation
          isOpen={this.state.isDialogDeleteConfirmationOpen}
          onClose={this._onClose_DialogDeleteConfirmation}
        />
      </div>
    )
  }
}
