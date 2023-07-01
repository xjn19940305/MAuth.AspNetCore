import api from '../index'
//文章
export default {
    Create: (data: any) => api.post('api/management/Article', data),
    Modify: (data: any) => api.put('api/management/Article', data),
    Delete: (data: string[]) => api.delete('api/management/Article', { data: data }),
    Get: (id: string) => api.get(`api/management/Article/${id}`),
    Query: (data: any) => api.get(`api/management/Article`, { params: data }),
}
