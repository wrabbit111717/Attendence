import http from 'src/services/http'

export const fetchPorts = async (query, signal) => {
  const params = new URLSearchParams()
  params.set('handler', 'PortList')
  params.set('query', query)
  const { data } = await http.get(`/?${params.toString()}`, { signal })
  return data
}

export const fetchPort = async portId => {
  const params = new URLSearchParams()
  params.set('handler', 'Port')
  params.set('portId', portId)
  const { data } = await http.get(`/?${params.toString()}`)
  return data
}

export default fetchPorts
