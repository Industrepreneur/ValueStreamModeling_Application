import * as React from 'react'

import { BrowserRouter as Router, Route, Link, Switch } from 'react-router-dom'

// import { PageHome } from './PageHome'
// import { PageAbout } from './PageAbout'

import { PageNav } from './PageNav'

import { InputLabor } from './mpx/InputLabor'
import { InputEquipment } from './mpx/InputEquipment'
import { InputProducts } from './mpx/InputProducts'
import { MpxReactTest } from './mpx/MpxReactTest'

const Routes = () => (
  <Router>
    <div>
      <Switch>
        {/* <Route exact={true} path="/" component={PageHome} />
        <Route path="/home" component={PageHome} />
        <Route path="/about" component={PageAbout} /> */}

         <Route path="/nav" component={PageNav} />

        <Route path="/input/labor(.aspx)?" component={InputLabor} />
        <Route path="/input/equipment(.aspx)?" component={InputEquipment} />
        <Route path="/input/products(.aspx)?" component={InputProducts} />
        <Route path="/input/test(.aspx)?" component={MpxReactTest} />

        <Route component={NoMatch} />
      </Switch>
    </div>
  </Router>
)

const NoMatch = ({ location }) => (
  <div>
    {/* <h3>
      No match for <code>{location.pathname}</code>
    </h3> */}
  </div>
)

export { Routes }
