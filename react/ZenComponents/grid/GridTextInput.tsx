import { React, _ } from './gridImports'
import { defaultColumnWidth, defaultColumnHeight } from './gridDefaults'

class GridTextInput extends React.Component<{}, {}> {
  render() {
    let width = 100
    let height = defaultColumnHeight
    return (
      <div style={{ width: width + 'px', height: height + 'px' }}>&nbsp;</div>
    )
  }
}


