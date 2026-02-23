import { setInnerHtml } from './domUtils.js'

/**
 * @param {HTMLElement | object | string} param
 * @param {HTMLElement} target
 */
export const parseHtmlToContainer = (param, target) => {
  if (param instanceof HTMLElement) {
    target.appendChild(param)
  }

  else if (typeof param === 'object') {
    handleObject(param, target)
  }

  else if (param) {
    setInnerHtml(target, param)
  }
}

/**
 * @param {any} param
 * @param {HTMLElement} target
 */
const handleObject = (param, target) => {
  if (param.jquery) {
    handleJqueryElem(target, param)
  }

  else {
    setInnerHtml(target, param.toString())
  }
}

/**
 * @param {HTMLElement} target
 * @param {any} elem
 */
const handleJqueryElem = (target, elem) => {
  target.textContent = ''
  if (0 in elem) {
    for (let i = 0; i in elem; i++) {
      target.appendChild(elem[i].cloneNode(true))
    }
  } else {
    target.appendChild(elem.cloneNode(true))
  }
}
