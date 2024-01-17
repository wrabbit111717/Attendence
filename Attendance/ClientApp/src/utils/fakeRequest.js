export default async function fakeRequest({ timeout = 1000, error = false }) {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      if (error) return reject()
      resolve({ data: null })
    }, timeout)
  })
}
