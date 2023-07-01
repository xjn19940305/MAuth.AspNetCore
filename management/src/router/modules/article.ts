import type { RouteRecordRaw } from 'vue-router'

function Layout() {
    return import('@/layouts/index.vue')
}

const routes: RouteRecordRaw = {
    path: '/article',
    component: Layout,
    redirect: '/article/index',
    name: 'article',
    meta: {
        title: '文章管理',
        icon: 'ep:collection',
    },
    children: [
        {
            path: 'index',
            name: 'article_index',
            component: () => import('@/views/article/index.vue'),
            meta: {
                title: '文章管理',
                sidebar: false,
                breadcrumb: false,
                auth: ['article.query']
            },
        },
        {
            path: 'detail',
            name: 'article_detail',
            component: () => import('@/views/article/detail.vue'),
            meta: {
                title: '文章详情',
                sidebar: false,                
                auth: []
            },
        },
    ],
}

export default routes
