import axios from 'axios'
import getCookie from 'src/utils/getCookie'

const origin = ''
const baseURL = `${origin}/Briefcases`

const headers = {
  Accept: 'application/json',
  'Content-Type': 'application/json'
}

const http = axios.create({
  baseURL,
  withCredentials: false,
  headers,
  responseType: 'json'
})

http.interceptors.request.use(request => {
  if (request.method === 'get') {
    return request
  }
  return {
    ...request,
    headers: {
      ...request.headers,
      RequestVerificationToken: getCookie('XSRF-TOKEN')
    }
  }
})

http.interceptors.response.use(
  response => response,
  async error => {
    if (error?.response?.status === 401) {
      window.location.href = '/Login'
    }
    return Promise.reject(error)
  }
)

export default http
