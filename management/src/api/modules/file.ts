import api from '@/api/index'

export default {
    // 登录
    upload: (name: string, data: any) => api.post(`api/file/CreateFile?name=${name}`, data, { headers: { 'Content-Type': data.type } }),
    getFile: (path: string) => {
        path = import.meta.env.VITE_APP_API_BASEURL + '/api/file/GetFileContent/' + encodeURIComponent(path)
        return path;
    }
}
