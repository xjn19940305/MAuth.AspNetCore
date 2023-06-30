import api from '../index'
//轮播图
export default {    
    Create: (data: any) => api.post('api/management/Carousel', data),
    Modify: (data: any) => api.put('api/management/Carousel', data),
    Delete: (data: string[]) => api.delete('api/management/Carousel', { data: data }),
    Get: (id: string) => api.get(`api/management/Carousel/${id}`),
    Query: (data: any) => api.get(`api/management/Carousel`, { params: data }),
}
