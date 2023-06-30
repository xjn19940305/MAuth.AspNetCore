import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/category',
    component: Layout,
    redirect: '/category/index',
    name: 'category',
    meta: {
        title: '分类管理',
        icon: 'ep:collection',
    },
    children: [
        {
            path: 'index',
            name: 'category_index',
            component: () => import('@/views/category/index.vue'),
            meta: {
                title: '分类管理',
                sidebar: false,
                breadcrumb: false,                
                auth: ['category.query',]
            },
        },
    ],
}

export default routes
