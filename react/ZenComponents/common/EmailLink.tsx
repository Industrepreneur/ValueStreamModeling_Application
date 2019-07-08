import React from 'react'

const EmailLink = (props: { email: string }) => {
  return <a href={'mailto:' + props.email}>{props.email}</a>
}

export { EmailLink }
