import api from '../index'

export default {
    // 登录
    Create: (data: any) => api.post('api/management/role', data),
    Modify: (data: any) => api.put('api/management/role', data),
    Delete: (data: string[]) => api.delete('api/management/role', { data: data }),
    Get: (id: string) => api.get(`api/management/role/${id}`),
    Query: (data: any) => api.get(`api/management/role`, { params: data }),
    Authorization: (roleId: string, data: string[]) => api.put(`api/management/role/Authorization/${roleId}`, data),
    GetRoleAuthorization: (roleId: string) => api.get(`api/management/role/Authorization/Get/${roleId}`),
}
