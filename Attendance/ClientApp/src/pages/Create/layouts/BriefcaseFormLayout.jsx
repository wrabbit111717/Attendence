import { useEffect, useMemo } from 'react'
import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import { Controller, useForm } from 'react-hook-form'
import ReactSelect from 'react-select'
import ButtonGroup from 'src/components/ButtonGroup/ButtonGroup'
import ErrorBoundary from 'src/components/ErrorBoundary/ErrorBoundary'
import PortSelector from 'src/components/PortSelector/PortSelector'
import SuccessBoundary from 'src/components/SuccessBoundary/SuccessBoundary'
import useBriefcase from 'src/pages/Create/context/useBriefcase'
import briefcaseDefaultValues from 'src/utils/briefcaseFakeValues'
import errorMessages from 'src/utils/errorMessages'
import isDev from 'src/utils/isDev'
import selectProps from 'src/utils/selectProps'

const BriefcaseFormLayout = props => {
  const {
    onSubmit,
    briefcase: { vessels, inspectionTypes, inspectionSources, questionnaires, data },
    error,
    loading,
    onReset,
    fakeFetch
  } = useBriefcase()
  const defaultValues = useMemo(
    () => ({
      id: null,
      userId: null,
      vesselId: null,
      inspectionTypeId: null,
      inspectionSourceId: null,
      inspectionCode: null,
      vettingDate: null,
      inspectorName: null,
      portId: null,
      comments: null,
      ...data,
      questionnaires: data?.questionnaires.map(q => q.qId) ?? []
    }),
    [data]
  )
  const {
    reset,
    handleSubmit,
    register,
    control,
    formState: { errors: validationErrors, isDirty, isSubmitted, isSubmitSuccessful }
  } = useForm({ defaultValues })
  const handleReset = () => {
    reset(defaultValues)
    onReset()
  }
  useEffect(() => {
    reset(defaultValues)
  }, [data])
  useEffect(() => {
    if (isDirty) {
      onReset()
    }
  }, [isDirty])
  // const handleWindowUnload = e => {
  //   console.log('fired')
  //   if (isDirty) {
  //     e.preventDefault()
  //     e.returnValue = ''
  //   } else {
  //     delete e.returnValue
  //   }
  // }
  // useEffect(() => {
  //   // TODO: fix this
  //   window.addEventListener('beforeunload', handleWindowUnload, true)
  //   return () => window.removeEventListener('beforeunload', handleWindowUnload)
  // }, [])
  const handleFocus = event => event.target.select()
  const isValid = name => isSubmitted && !isSubmitSuccessful && !validationErrors[name]
  const isInvalid = name => isSubmitted && !isSubmitSuccessful && !!validationErrors[name]
  const validationMessage = name => validationErrors[name]?.message
  const required = { required: errorMessages.validation.required }
  const a11yProps = (name, options) => ({
    'aria-invalid': isInvalid(name),
    'aria-describedby': `${name}-feedback`,
    'aria-errormessage': `${validationErrors[name]?.message ?? ''}`,
    custom: true,
    id: name,
    isValid: isValid(name),
    isInvalid: isInvalid(name),
    ...register(name, { ...options })
  })
  const handleDebug = async () => {
    reset(briefcaseDefaultValues(data))
    await fakeFetch({ error: true, timeout: 2200 })
  }
  return (
    <Container {...props}>
      <Form onSubmit={handleSubmit(onSubmit)}>
        {error === false && (
          <SuccessBoundary onClose={onReset} message="Briefcase has been successfully updated!" />
        )}
        {error && <ErrorBoundary onClose={onReset} message={error} />}
        <fieldset disabled={loading}>
          <Row>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="vesselId">Vessel Name</Form.Label>
              <Form.Control {...a11yProps('vesselId', required)} as="select">
                <option value="">Select Vessel Name</option>
                {vessels.map(({ id, name }) => (
                  <option key={id} value={id}>
                    {name}
                  </option>
                ))}
              </Form.Control>
              <Form.Control.Feedback type="invalid">
                {validationMessage('vesselId')}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="inspectionTypeId">Inspection Type</Form.Label>
              <Form.Control {...a11yProps('inspectionTypeId', required)} as="select">
                <option value="">Select Inspection Type</option>
                {inspectionTypes.map(({ id, name }) => (
                  <option key={id} value={id}>
                    {name}
                  </option>
                ))}
              </Form.Control>
              <Form.Control.Feedback type="invalid">
                {validationMessage('inspectionTypeId')}
              </Form.Control.Feedback>
            </Form.Group>
          </Row>
          <Row>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="inspectionSourceId">Inspection Source</Form.Label>
              <Form.Control {...a11yProps('inspectionSourceId', required)} as="select">
                <option value="">Select Inspection Source</option>
                {inspectionSources.map(({ id, name }) => (
                  <option key={id} value={id}>
                    {name}
                  </option>
                ))}
              </Form.Control>
              <Form.Control.Feedback type="invalid">
                {validationMessage('inspectionSourceId')}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="vettingDate">Vetting Date</Form.Label>
              <Form.Control
                {...a11yProps('vettingDate', required)}
                className="form-control"
                type="datetime-local"
              />
              <Form.Control.Feedback type="invalid">
                {validationMessage('vettingDate')}
              </Form.Control.Feedback>
            </Form.Group>
          </Row>
          <Row>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="inspectorName">Inspector Name</Form.Label>
              <Form.Control
                {...a11yProps('inspectorName', {
                  maxLength: { value: 50, message: errorMessages.validation.maxLength(50) }
                })}
                className="form-control"
                type="text"
                onFocus={handleFocus}
              />
              <Form.Control.Feedback type="invalid">
                {validationMessage('inspectorName')}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="portId">Port</Form.Label>
              <Controller
                rules={{ ...required }}
                control={control}
                name="portId"
                render={({ field: { onChange, value, ref } }) => (
                  <PortSelector
                    onFocus={handleFocus}
                    error={validationMessage('portId')}
                    isValid={isValid('portId')}
                    isInvalid={isInvalid('portId')}
                    id="portId"
                    ref={ref}
                    onChange={onChange}
                    value={value}
                  />
                )}
              />
            </Form.Group>
          </Row>
          <Row>
            <Form.Group as={Col} className="my-2">
              <Form.Label htmlFor="comments">Comments</Form.Label>
              <Form.Control
                {...a11yProps('comments')}
                as="textarea"
                rows={3}
                className="form-control"
                onFocus={handleFocus}
              />
              <Form.Control.Feedback type="invalid">
                {validationMessage('comments')}
              </Form.Control.Feedback>
            </Form.Group>
          </Row>
          <Row>
            <Form.Group as={Col} className="my-2">
              <Form.Label htmlFor="questionnaires">Questionnaires</Form.Label>
              <Controller
                rules={{ ...required }}
                control={control}
                name="questionnaires"
                defaultValue={[]}
                render={({ field: { onChange, value, ...field } }) => {
                  const handleChange = (nextValue, { action, removedValue }) => {
                    if (action === 'select-option') {
                      return onChange(nextValue.map(option => option.qId))
                    }
                    if (action === 'remove-value') {
                      return onChange(value.filter(qId => qId !== removedValue.qId))
                    }
                    if (action === 'clear') {
                      return onChange([])
                    }
                  }
                  return (
                    <ReactSelect
                      {...field}
                      isDisabled={loading}
                      value={questionnaires.filter(option => value.includes(option.qId))}
                      onChange={handleChange}
                      {...selectProps({
                        isValid: isValid('questionnaires'),
                        isInvalid: isInvalid('questionnaires')
                      })}
                      aria-invalid={isInvalid('questionnaires')}
                      aria-describedby="questionnaires-feedback"
                      aria-errormessage={`${validationErrors.questionnaires?.message ?? ''}`}
                      id="questionnaires"
                      menuPosition="fixed"
                      menuPlacement="top"
                      placeholder="Questionnaires"
                      getOptionLabel={option => option.title}
                      closeMenuOnSelect={false}
                      hideSelectedOptions={false}
                      isOptionSelected={(option, selectValue) =>
                        selectValue.some(({ qId }) => qId === option.qId)
                      }
                      options={questionnaires}
                      isMulti
                    />
                  )
                }}
              />
              {isInvalid('questionnaires') && (
                <Form.Control.Feedback
                  style={{
                    display: 'block'
                  }}
                  type="invalid">
                  {validationMessage('questionnaires')}
                </Form.Control.Feedback>
              )}
            </Form.Group>
          </Row>
          <Row>
            <Col>
              <ButtonGroup className="my-3">
                <Button disabled={!isDirty} type="submit" variant="primary">
                  {loading ? 'Please wait...' : 'Submit'}
                </Button>
                <Button onClick={handleReset} variant="secondary">
                  Reset
                </Button>
                {isDev() && (
                  <Button onClick={handleDebug} variant="danger">
                    Debug
                  </Button>
                )}
              </ButtonGroup>
            </Col>
          </Row>
        </fieldset>
      </Form>
    </Container>
  )
}

export default BriefcaseFormLayout
