// Create a namespaced logger
let consoleLog: any = (...args) => {
  if (typeof window !== 'undefined') {
    if (window && window.console && window.console.log) {
      // tslint:disable-next-line
      window.console.log(...args)
    }
  } else if (typeof console !== 'undefined') {
    if (console && console.log) {
      // tslint:disable-next-line
      console.log(...args)
    }
  }
}

let consoleLogEx: any = (funcName, ...args) => {
  if (typeof window !== 'undefined') {
    if (window && window.console && window.console.log) {
      if (window.console[funcName]) {
        // tslint:disable-next-line
        window.console[funcName](...args)
      } else {
        // tslint:disable-next-line
        window.console.log(...args)
      }
    }
  } else if (typeof console !== 'undefined') {
    if (console && console.log) {
      if (console[funcName]) {
        // tslint:disable-next-line
        console[funcName](...args)
      } else {
        // tslint:disable-next-line
        console.log(...args)
      }
    }
  }
}

export class Logger {
  constructor(private name = 'default') {}

  nil = (...args) => {
    // do nothing
  }
  x = (...args) => {
    consoleLog(':)', this.name, ...args)
  }
  info = (...args) => {
    consoleLogEx('info', 'B)', this.name, ...args)
  }
  ok = (...args) => {
    consoleLog(':D', this.name, ...args)
  }
  warn = (...args) => {
    consoleLogEx('warn', ':/', this.name, ...args)
  }
  error = (...args) => {
    consoleLogEx('error', ':(', this.name, ...args)
  }
  catastrophicError = (...args) => {
    consoleLogEx('error', 'D: CATASTROPHIC ERROR', this.name, ...args)
  }
  debug = (...args) => {
    consoleLogEx('debug', ':P', this.name, ...args)
  }
  dev = (...args) => {
    consoleLogEx('debug', ':p', this.name, ...args)
  }
  bare = (...args) => {
    consoleLog(...args)
  }
  json = obj => {
    if (obj === null) {
      obj = 'null'
    }
    if (obj === undefined) {
      obj = 'undefined'
    }
    consoleLog(JSON.stringify(obj, null, 2))
  }
}

export function createLogger(name) {
  return new Logger(name)
}

export function test() {
  let l = createLogger('test')
  l.nil('nil')
  l.x('x')
  l.info('info')
  l.ok('ok')
  l.warn('warn')
  l.error('error')
  l.catastrophicError('catastrophicError')
  l.debug('debug')
  l.dev('dev')
  l.bare('bare')
}
