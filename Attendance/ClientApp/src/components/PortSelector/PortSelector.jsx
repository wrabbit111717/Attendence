import React from 'react'
import { Form } from 'react-bootstrap'
import { AsyncTypeahead } from 'react-bootstrap-typeahead'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'
// eslint-disable-next-line import/no-unresolved
import fetchPorts, { fetchPort } from 'src/services/actions/port'

const PortSelector = React.forwardRef(
  ({ value: inputValue, onChange, className, error, isValid, isInvalid, ...props }, ref) => {
    const [ports, setPorts] = React.useState([])
    const [value, setValue] = React.useState([])
    const [loading, setLoading] = React.useState(false)
    const handleChange = selected => setValue(selected)
    const handleInput = query => {
      setLoading(true)
      fetchPorts(query)
        .then(data => setPorts(data))
        .finally(() => setLoading(false))
    }
    const getPort = async portId => {
      setLoading(true)
      try {
        const data = await fetchPort(portId)
        return data
      } catch {
        return null
      } finally {
        setLoading(false)
      }
    }
    React.useEffect(() => {
      if (inputValue === value?.id) return
      if (!inputValue) {
        setValue([])
        return
      }
      getPort(inputValue).then(data => {
        let value = ports.find(port => port.id === data?.id)
        if (!value) {
          if (data) {
            ports.push(data)
            value = data
          }
        }
        setValue(value ? [value] : [])
      })
    }, [inputValue])

    React.useEffect(() => {
      onChange?.(value.at(0)?.id ?? null)
    }, [value])

    return value ? (
      <AsyncTypeahead
        ref={ref}
        labelKey={option => {
          return `${option.name} (${option.country})`
        }}
        clearButton
        isLoading={loading}
        onChange={handleChange}
        options={ports}
        filterBy={() => true}
        selected={value}
        onSearch={handleInput}
        renderMenuItemChildren={option => (
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '25% 25% 25% 25% 25%',
              gridGap: '0 15px'
            }}>
            <div>
              <span className="fw-bold">{option.name?.trim()}</span>
            </div>
            <div>{option.code?.trim()}</div>
            <div>{option.country?.trim()}</div>
            <div>
              <button
                className="btn btn-link px-0 mx-0 py-0 my-0"
                size="small"
                type="button"
                onClick={e => {
                  e.stopPropagation()
                  e.preventDefault()
                  window.open(
                    `https://www.google.com/maps/place/${option.lat},${option.lng}/@${option.lat},${option.lng},12z`,
                    '_blank'
                  )
                }}>
                View
              </button>
            </div>
          </div>
        )}
        renderInput={({ inputRef, referenceElementRef, ...inputProps }) => (
          <>
            <Form.Control
              {...inputProps}
              ref={input => {
                inputRef(input)
                referenceElementRef(input)
              }}
              className={className}
              isValid={isValid}
              isInvalid={isInvalid}
            />
            <Form.Control.Feedback type="invalid">{error}</Form.Control.Feedback>
          </>
        )}
        {...props}
      />
    ) : null
  }
)

PortSelector.displayName = 'PortSelector'

export default PortSelector
