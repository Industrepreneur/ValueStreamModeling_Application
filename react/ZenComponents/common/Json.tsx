import React from 'react'

const Json = props => (
  <div
    style={{
      padding: '4px',
      margin: '4px',
      overflow: 'auto',
      border: 'solid 1px #ccc',
      borderRadius: '8px',
    }}
  >
    <pre>{JSON.stringify(props.data || 'no data :P', null, 2)}</pre>
  </div>
)

export { Json }
