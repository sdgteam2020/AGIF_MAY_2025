import globalState from '../globalState.js'
import privateMethods from '../privateMethods.js'
import privateProps from '../privateProps.js'

/**
 * Dispose the current SweetAlert2 instance
 */
export function _destroy() {
  const domCache = privateProps.domCache.get(this)
  const innerParams = privateProps.innerParams.get(this)

  if (!innerParams) {
    disposeWeakMaps(this) // The WeakMaps might have been partly destroyed, we must recall it to dispose any remaining WeakMaps #2335
    return // This instance has already been destroyed
  }

  if (domCache.popup && globalState.swalCloseEventFinishedCallback) {
    globalState.swalCloseEventFinishedCallback()
    delete globalState.swalCloseEventFinishedCallback
  }

  if (typeof innerParams.didDestroy === 'function') {
    innerParams.didDestroy()
  }
  globalState.eventEmitter.emit('didDestroy')
  disposeSwal(this)
}

/**
 * @param {SweetAlert} instance
 */
const disposeSwal = (instance) => {
  disposeWeakMaps(instance)
  delete instance.params
  delete globalState.keydownHandler
  delete globalState.keydownTarget
  delete globalState.currentInstance
}

/**
 * @param {SweetAlert} instance
 */
const disposeWeakMaps = (instance) => {
  if (instance.isAwaitingPromise) {
    unsetWeakMaps(privateProps, instance)
    instance.isAwaitingPromise = true
  } else {
    unsetWeakMaps(privateMethods, instance)
    unsetWeakMaps(privateProps, instance)

    delete instance.isAwaitingPromise
    delete instance.disableButtons
    delete instance.enableButtons
    delete instance.getInput
    delete instance.disableInput
    delete instance.enableInput
    delete instance.hideLoading
    delete instance.disableLoading
    delete instance.showValidationMessage
    delete instance.resetValidationMessage
    delete instance.close
    delete instance.closePopup
    delete instance.closeModal
    delete instance.closeToast
    delete instance.rejectPromise
    delete instance.update
    delete instance._destroy
  }
}

/**
 * @param {object} obj
 * @param {SweetAlert} instance
 */
const unsetWeakMaps = (obj, instance) => {
  for (const i in obj) {
    obj[i].delete(instance)
  }
}
