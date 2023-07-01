<template>
    <div>
        <pageMain title="查询">
            <search-bar :fold="isFold">
                <template #default="{ fold }">
                    <el-form :model="queryFormModel" size="default" label-width="120px">
                        <el-row>
                            <el-col :span="8">
                                <el-form-item label="标题">
                                    <el-input @keydown.enter="query" v-model="queryFormModel.title" placeholder="请输入标题"
                                        clearable />
                                </el-form-item>
                            </el-col>
                            <el-col :span="8">
                                <el-form-item label="作者">
                                    <el-input @keydown.enter="query" v-model="queryFormModel.author" placeholder="请输入作者"
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
        <pageMain title="数据列表">
            <el-row class="mb-4">
                <el-button type="primary" @click="Create">添加</el-button>
            </el-row>
            <el-table v-loading="table.loading" :data="table.data" stripe border style="width: 100%;margin-top:15px;"
                height="100%">
                <el-table-column prop="id" label="id" />
                <el-table-column prop="title" label="标题" />
                <el-table-column prop="author" label="作者" />
                <el-table-column prop="filePath" label="首图" align="center">
                    <template #default="scope">
                        <el-image style="width: 100px; height: 100px" :src="file.getFile(scope.row.picture)"
                            :zoom-rate="1.2" :preview-teleported="true"
                            :preview-src-list="[file.getFile(scope.row.picture)]" :initial-index="0" fit="cover" />
                    </template>
                </el-table-column>
                <el-table-column prop="dateCreated" label="创建日期" align="center">
                    <template #default="scope">
                        {{ moment.utc(scope.row.dateCreated).local().format("YYYY-MM-DD HH:mm") }}
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center">
                    <template #default="scope">
                        <el-row class="mb-4">
                            <el-button type="primary" @click="Modify(scope.row.id)" v-auth="'role.edit'">修改</el-button>
                            <el-button type="danger" v-auth="'role.delete'" @click="Delete([scope.row.id])">删除</el-button>
                        </el-row>
                    </template>
                </el-table-column>
            </el-table>
            <div style="margin-top: 30px; display: flex; justify-content: right;">
                <el-pagination background v-model:current-page="table.paging.page" v-model:page-size="table.paging.pageSize"
                    :page-sizes="table.pageArray" layout="total, sizes, prev, pager, next, jumper" :total="table.total"
                    @size-change="HandleSizeChange" @current-change="handleCurrentChange" />
            </div>
        </pageMain>


    </div>
</template>
  
<script lang="ts" setup>
import { ElMessage } from 'element-plus'
import api from '@/api/modules/article'
import file from '@/api/modules/file'
import moment from 'moment'
import { useRouter } from 'vue-router'
import { ref, reactive, onMounted } from 'vue'
import { Plus } from '@element-plus/icons-vue'
const router = useRouter()

const isFold = false;
let queryFormModel = ref({
    title: '',
    author: ''
})

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

const HandleSizeChange = async (value: any) => {
    table.paging!.pageSize = value
    await query();
}
const handleCurrentChange = async (value: any) => {
    table.paging!.page = value
    await query();
}
onMounted(async () => {
    console.log('mounted')
    await query();
})

const query = async () => {
    table.loading = true;
    let queryParams = Object.assign(table.paging, queryFormModel.value)
    let res = (await api.Query(queryParams)) as any;
    table.data = res.data
    table.loading = false
    table.total = res.totalElements
    console.log(res)
    console.log(table.paging)
}
const Delete = async (ids: string[]) => {
    console.log(ids)
    await api.Delete(ids)
    await query();
    ElMessage.success('删除成功!')
}
const Create = () => {
    router.push("/article/detail");
}
const Modify = (id: string) => {
    router.push("/article/detail?id=" + id);
}

</script>

<style scoped>
.avatar-uploader .avatar {
    width: 178px;
    height: 178px;
    display: block;
}
</style>

<style>
.avatar-uploader .el-upload {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
}

.avatar-uploader .el-upload:hover {
    border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    text-align: center;
}
</style>