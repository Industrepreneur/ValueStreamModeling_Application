import * as React from 'react'

import {
  _,
} from '@zen-components/grid/gridExports'

import Input, { InputLabel } from 'material-ui/Input'
import { MenuItem } from 'material-ui/Menu'
import { FormControl, FormHelperText } from 'material-ui/Form'
import Select from 'material-ui/Select'

import * as mpxTableUtil from './mpxTableUtil'

export class ProductFilter extends React.Component<{
  allProductNames: any,
  selectedProductName: string,
  onChange: (newProductName: string) => void,
}> {

  state = {
    allProductNames: [] as string[],
  }
  // _fetch = async () => {
  //   let allProductNames = await mpxTableUtil.selectList('products', 'proddesc')
  //   this.setState({ allProductNames: allProductNames })
  // }
  // componentWillMount() {
  //   this._fetch()
  // }

  _onChange = (event) => {
    this.props.onChange(event.target.value)
  }

  render() {
    return (
      <div style={{ padding: '10px' }}>

        <FormControl>
          <InputLabel htmlFor='product-simple'>Product</InputLabel>
          <Select
            style={{ minWidth: '160px' }}
            value={this.props.selectedProductName}
            onChange={this._onChange}
            inputProps={{
              name: 'product',
              id: 'product-simple',
            }}
          >
            {/* <MenuItem key='-' value="-">
              <em>Please select a product</em>
            </MenuItem> */}

            {/* <MenuItem key='empty' value="">

            </MenuItem> */}
            {
              _.map(this.props.allProductNames, (c) => (
                <MenuItem key={c} value={c}>{c}</MenuItem>
              ))
            }
          </Select>
        </FormControl>

      </div>
    )
  }
}
