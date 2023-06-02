<template>
    <div>
        <pageMain v-auth="'user.search'">
            <search-bar :fold="isFold">
                <template #default="{ fold }">
                    <el-form :model="queryFormModel" size="default" label-width="120px">
                        <el-row>
                            <el-col :span="8">
                                <el-form-item label="手机号">
                                    <el-input @keydown.enter="query" v-model="queryFormModel.phoneNumber"
                                        placeholder="请输入手机号" clearable />
                                </el-form-item>
                            </el-col>
                            <el-col :span="8">
                                <el-form-item label="账号">
                                    <el-input @keydown.enter="query" v-model="queryFormModel.userName" placeholder="请输入账号"
                                        clearable />
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-form-item>
                            <el-button type="primary" @click="query">
                                <template #icon>
                                    <el-icon>
                                        <svg-icon name="ep:search" />
                                    </el-icon>
                                </template>
                                查询
                            </el-button>
                        </el-form-item>
                    </el-form>
                </template>
            </search-bar>
        </pageMain>
        <PageMain>
            <el-row class="mb-4">
                <el-button type="primary" @click="Create" v-auth="'user.create'">添加</el-button>
            </el-row>
            <el-table v-loading="table.loading" :data="table.data" stripe border style="width: 100%;margin-top:15px;"
                height="100%">
                <el-table-column prop="id" label="id" />
                <el-table-column prop="userName" label="用户名" />
                <el-table-column prop="nickName" label="昵称" />
                <el-table-column prop="phoneNumber" label="手机号" align="center" />
                <el-table-column prop="email" label="邮箱" align="center" />
                <el-table-column prop="dateCreated" label="创建日期" align="center">
                    <template #default="scope">
                        {{ moment.utc(scope.row.dateCreated).local().format("YYYY-MM-DD HH:mm") }}
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center">
                    <template #default="scope">
                        <el-row class="mb-4">
                            <el-button type="primary" @click="Modify(scope.row.id)" v-auth="'user.edit'">修改</el-button>
                            <el-button type="primary" @click="BindRole(scope.row.id)"
                                v-auth="'user.bindrole'">绑定角色</el-button>
                            <el-button type="danger" v-auth="'user.delete'" @click="Delete([scope.row.id])">删除</el-button>
                        </el-row>
                    </template>
                </el-table-column>
            </el-table>
            <div style="margin-top: 30px; display: flex; justify-content: right;">
                <el-pagination background v-model:current-page="table.paging.page" v-model:page-size="table.paging.pageSize"
                    :page-sizes="table.pageArray" layout="total, sizes, prev, pager, next, jumper" :total="table.total"
                    @size-change="HandleSizeChange" @current-change="handleCurrentChange" />
            </div>
        </PageMain>
        <el-drawer v-model="dialog" title="数据操作" direction="rtl" :before-close="handleClose">
            <el-form label-position="top" label-width="100px" :model="formModel" :rules="rules" ref="roleFormRef"
                @keyup.enter="onSubmit">
                <el-form-item label="用户名" prop="userName">
                    <el-input v-model="formModel.userName" />
                </el-form-item>
                <el-form-item label="昵称" prop="nickName">
                    <el-input v-model="formModel.nickName" />
                </el-form-item>
                <el-form-item label="手机号" prop="phoneNumber">
                    <el-input v-model="formModel.phoneNumber" />
                </el-form-item>
                <el-form-item label="邮箱" prop="email">
                    <el-input v-model="formModel.email" />
                </el-form-item>
                <el-form-item label="密码" prop="password">
                    <el-input type="password" v-model="formModel.password" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="onSubmit">保存</el-button>
                    <el-button @click="cancel">取消</el-button>
                </el-form-item>
            </el-form>
        </el-drawer>
        <roleSelectComponet v-model:show="selectRoleData.bindRuleShow" v-model:userId="selectRoleData.selectUserId">
        </roleSelectComponet>
    </div>
</template>
  
<script lang="ts" setup>
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import api from '@/api/modules/user'
import moment from 'moment'
const roleSelectComponet = defineAsyncComponent(() => import('./component/selectRole.vue'))
const table = reactive<any>({
    loading: false,
    data: [],
    pageArray: [20, 50, 100],
    total: 1,
    paging: {
        pageSize: 10,
        page: 1
    }
})
const isFold = false;
const dialog = ref(false)
const roleFormRef = ref<FormInstance>()
let queryFormModel = ref({
    userName: '',
    phoneNumber: ''
})
const formModel = ref({
    id: '',
    userName: '',
    phoneNumber: '',
    nickName: null,
    password: '',
    email: '',
})
const rules = reactive<FormRules>({
    userName: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
})
const selectRoleData = reactive({
    bindRuleShow: false,
    selectUserId: ''
})


onMounted(async () => {
    console.log('mounted')
    await query();
})
const HandleSizeChange = async (value: any) => {
    table.paging!.pageSize = value
    await query();
}
const handleCurrentChange = async (value: any) => {
    table.paging!.page = value
    await query();
}

const BindRole = (id: string) => {
    selectRoleData.selectUserId = id
    selectRoleData.bindRuleShow = true
}

const query = async () => {
    table.loading = true
    let queryParams = Object.assign(table.paging, queryFormModel.value)
    let res = (await api.Query(queryParams)) as any;
    table.data = res.data
    table.loading = false
    table.total = res.totalElements
    console.log(res)
    console.log(table.paging)
}

const handleClose = (done: () => void) => {
    roleFormRef.value!.resetFields()
    formModel.value!.id = ''
    done()
}
const Create = async () => {
    dialog.value = true
}
const Modify = async (id: string) => {
    dialog.value = true
    let res = (await api.Get(id)) as any
    formModel.value = res
    console.log(id)
}
const Delete = async (ids: string[]) => {
    console.log(ids)
    await api.Delete(ids)
    await query();
    ElMessage.success('删除成功!')
}
const onSubmit = async () => {
    await roleFormRef.value!.validate(async (valid, fields) => {
        if (valid) {            
            if (formModel.value.id == '') {
                delete formModel.value!.id
                let res = await api.Create(formModel.value)
                roleFormRef.value!.resetFields()
                dialog.value = false
                ElMessage.success("添加成功!")
            } else {
                let res = await api.Modify(formModel.value)
                roleFormRef.value!.resetFields()
                dialog.value = false
                ElMessage.success("修改成功!")
            }
            formModel.value!.id = ''
            await query()
        }
    })

}
const cancel = () => {
    roleFormRef.value!.resetFields()
    formModel.value!.id = ''
    dialog.value = false
}
</script>