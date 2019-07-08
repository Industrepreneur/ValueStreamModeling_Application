import * as React from 'react'
import * as _ from 'lodash'

let isVerbose = false

export class TextInput extends React.Component<
  {
    text: string
    tag?: any
    onUpdate: (newValue: any, tag: any) => void
  },
  {}
> {
  state = {
    isEditing: false,
    editValue: '',
  }

  componentWillUnmount() {
    this._tryCommitChange()
  }
  onFocus = () => {
    this._startEditing()
  }
  onBlur = () => {
    this._tryCommitChange()
  }

  _startEditing = () => {
    // Start editing
    console.log('start editing')
    this.setState({ editValue: this.props.text || '', isEditing: true })
  }

  _tryCommitChange = () => {
    // Stop editing and commit change
    let newValue = this.state.editValue
    if (newValue && this.state.isEditing) {
      console.log('commit edit')
      if (newValue !== this.props.text) {
        this.setState({ isEditing: false })
        this.props.onUpdate(newValue, this.props.tag)
      }
    }
  }

  onChange = event => {
    let newValue = event.target.value
    if (isVerbose) {
      console.log('change to ', newValue)
    }
    this.setState({ editValue: newValue })
  }

  render() {
    return (
      <input
        type="text"
        onFocus={this.onFocus}
        onBlur={this.onBlur}
        onChange={this.onChange}
        style={{}}
        value={
          this.state.isEditing
            ? this.state.editValue || ''
            : this.props.text || ''
        }
      />
    )
  }
}
