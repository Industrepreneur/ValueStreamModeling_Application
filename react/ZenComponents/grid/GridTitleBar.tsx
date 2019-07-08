import { React, _ } from './gridImports'

export class GridTitleBar extends React.Component<
  {
    title: string
  },
  {}
> {
  render() {
    let { title } = this.props

    return (
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          padding: '10px',
          fontFamily: 'Roboto',
          fontWeight: 'bold',
        }}
      >
        <div>{title}</div>
        <div>{this.props.children}</div>
      </div>
    )
  }
}
