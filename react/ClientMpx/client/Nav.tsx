import * as React from 'react'
import * as _ from 'lodash'
import { Link } from 'react-router-dom'

let groups = [
  {
    label: 'MPX',
    links: [
      // {
      //   label: 'Home',
      //   link: '/home',
      // },
      {
        label: 'Nav',
        link: '/nav',
      },
      {
        label: 'Equipment',
        link: '/input/equipment',
      },
      {
        label: 'Products',
        link: '/input/products',
      },
      {
        label: 'Labor',
        link: '/input/labor',
      },
      {
        label: 'Test',
        link: '/input/test',
      },
    ],
  },
]

const Nav = () => (
  <div style={{ marginLeft: '10px' }}>
    {_.map(groups, (group, groupIdx) => (
      <div key={groupIdx}>
        <div>{group.label}</div>
        <ul style={{ margin: '0' }}>
          {_.map(group.links, (link, linkIdx) => (
            <li key={linkIdx}>
              <Link to={link.link}>{link.label}</Link>
            </li>
          ))}
        </ul>
      </div>
    ))}
  </div>
)
export { Nav }
