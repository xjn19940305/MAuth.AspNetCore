import api from '../index'
//分类
export default {
    Create: (data: any) => api.post('api/management/Category', data),
    Modify: (data: any) => api.put('api/management/Category', data),
    Delete: (data: string[]) => api.delete('api/management/Category', { data: data }),
    Get: (id: string) => api.get(`api/management/Category/${id}`),
    GetAllCategories: () => api.get(`api/management/Category`),
}
