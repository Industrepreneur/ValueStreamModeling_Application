import * as React from 'react'

import { Router, Route, Link, Switch } from 'react-router-dom'

// import { PageHome } from './PageHome'
// import { PageAbout } from './PageAbout'

import { PageNav } from './PageNav'

import { InputLabor } from './mpx/InputLabor'
import { InputEquipment } from './mpx/InputEquipment'
import { InputProducts } from './mpx/InputProducts'
import { InputProductsOperations } from './mpx/InputProductsOperations'
import { InputProductsRoutings } from './mpx/InputProductsRoutings'
import { InputProductsIbom } from './mpx/InputProductsIbom'
import { MpxDataDump } from './mpx/MpxDataDump'

import { config } from './config'

const Routes = () => (
  <Router history={config.history}>
    <div>
      <Switch>
        {/* <Route exact={true} path="/" component={PageHome} />
        <Route path="/home" component={PageHome} />
        <Route path="/about" component={PageAbout} /> */}

        <Route path="/nav" component={PageNav} />

        <Route path="/input/labor(.aspx)?" component={InputLabor} />
        <Route path="/input/equipment(.aspx)?" component={InputEquipment} />
        <Route path="/input/products(.aspx)?" component={InputProducts} />
        <Route path="/input/products-operations(.aspx)?" component={InputProductsOperations} />
        <Route path="/input/products-routings(.aspx)?" component={InputProductsRoutings} />
        <Route path="/input/products-ibom(.aspx)?" component={InputProductsIbom} />
        <Route path="/data-dump" component={MpxDataDump} />

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
