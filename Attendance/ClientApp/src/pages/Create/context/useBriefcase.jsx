import { createContext, useContext, useMemo, useState } from 'react'
import { useNavigate, useRevalidator } from 'react-router-dom'
import * as api from 'src/services/actions/briefcase'
import errorMessages from 'src/utils/errorMessages'
import fakeRequest from 'src/utils/fakeRequest'

const BriefcaseContext = createContext({})

export const BriefcaseProvider = ({ children, briefcase }) => {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const navigate = useNavigate()
  const { revalidate } = useRevalidator()
  const onReset = () => setError(null)
  const onSubmit = async formData => {
    onReset()
    setLoading(true)
    if (briefcase?.data?.id) {
      try {
        await api.updateBriefcase(briefcase.data.id, formData)
        setError(false)
        revalidate()
        return true
      } catch (error) {
        setError(errorMessages.api.update)
      } finally {
        setLoading(false)
      }
    } else {
      try {
        const id = await api.createBriefcase(formData)
        setError(false)
        navigate(`/`, { replace: true })
        return true
      } catch (error) {
        setError(errorMessages.api.create)
      } finally {
        setLoading(false)
      }
    }
    return false
  }
  const fakeFetch = async ({ timeout = 1000, error = false }) => {
    onReset()
    setLoading(true)
    try {
      const { data } = await fakeRequest({ timeout, error })
      setError(false)
    } catch {
      setError(errorMessages.api.unknown)
    } finally {
      setLoading(false)
    }
  }
  const onDelete = () => navigate('/', { replace: true })
  const memoedValue = useMemo(
    () => ({ briefcase, onSubmit, loading, error, onDelete, onReset, fakeFetch }),
    [briefcase, loading, error]
  )
  return <BriefcaseContext.Provider value={memoedValue}>{children}</BriefcaseContext.Provider>
}

const useBriefcase = () => useContext(BriefcaseContext)

export default useBriefcase
