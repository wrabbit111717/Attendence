import http from 'src/services/http'

export const fetchBriefcase = async briefcaseId => {
  const params = new URLSearchParams()
  params.set('handler', 'Briefcase')
  params.set('briefcaseId', briefcaseId ?? '')
  const { data } = await http.get(`/?${params.toString()}`)
  return data
}

export const fetchBriefcases = async signal => {
  const params = new URLSearchParams()
  params.set('handler', 'Briefcases')
  const { data } = await http.get(`/?${params.toString()}`, { signal })
  return data
}

export const createBriefcase = async briefcase => {
  const params = new URLSearchParams()
  params.set('handler', 'Create')
  const { data } = await http.post(`/?${params.toString()}`, briefcase)
  return data
}

export const updateBriefcase = async (briefcaseId, briefcase) => {
  const params = new URLSearchParams()
  params.set('handler', 'Update')
  params.set('briefcaseId', briefcaseId)
  const { data } = await http.put(`/?${params.toString()}`, briefcase)
  return data
}
