import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/carousel',
    component: Layout,
    redirect: '/carousel/index',
    name: 'carousel',
    meta: {
        title: '轮播图',
        icon: 'ep:collection',
    },
    children: [
        {
            path: 'index',
            name: 'carousel_index',
            component: () => import('@/views/carousel/index.vue'),
            meta: {
                title: '轮播图管理',
                sidebar: false,
                breadcrumb: false,                
                auth: ['carousel.query',]
            },
        },
    ],
}

export default routes
