let isDev = false
if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
  isDev = true
}

import createBrowserHistory from 'history/createBrowserHistory'
const history = createBrowserHistory()

function navigateTo(newLink) {
  console.log('navigate to', newLink)
  history.push(newLink)
}

function navigateBack() {
  history.goBack()
}

const config = {
  isDev,
  isProd: !isDev,
  history,
  navigateTo,
  navigateBack,
}

export { config }
