<template>
    <page-main>
        <el-button type="primary" @click="test">
            调用API test
        </el-button>
        <el-button type="primary" @click="test2">
            调用API test2
        </el-button>
        <el-button type="primary" @click="test3">
            调用API test3
        </el-button>
        <Auth :value="'demo.demo1'">
            <p>有权限则可以看到这里的demo.demo1</p>
        </Auth>
        <Auth :value="'demo.demo2'">
            <p>有权限则可以看到这里的demo.demo2</p>
        </Auth>
        <Auth :value="['demo.demo1', 'demo.demo2']">
            <p>demo1或者demo2有一个权限你就可以看到</p>
        </Auth>
        <AuthAll :value="['demo.demo1', 'demo.demo2']">
            <p>同时拥有demo1和demo2的权限你可以看到</p>

        </AuthAll>
    </page-main>
</template>
<script lang="ts" setup>
import api from '@/api/index'
import { ElMessage } from 'element-plus'
function test() {
    api.get('api/test/demo').then((res: any) => {
        ElMessage({
            message: res,
            type: 'success',
        })
    })
}
function test2() {
    api.get('api/test/demo2').then((res: any) => {
        ElMessage({
            message: res,
            type: 'success',
        })
    })
}
function test3() {
    api.get('api/test/demo3').then((res: any) => {
        ElMessage({
            message: res,
            type: 'success',
        })
    })
}
</script>