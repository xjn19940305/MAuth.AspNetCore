<template>
    <pageMain>
        <search-bar :fold="isFold">
            <template #default="{ fold }">
                <el-form :model="queryFormModel" size="default" label-width="120px">
                    <el-row>
                        <el-col :span="12">
                            <el-form-item label="角色名称">
                                <el-input v-model="queryFormModel.name" placeholder="请输入角色名称" clearable />
                            </el-form-item>
                        </el-col>
                    </el-row>
                    <el-form-item>
                        <el-button type="primary">
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
        <el-table :data="tableData" border style="width: 100%;margin-top:50px;">
            <el-table-column prop="date" label="Date" width="180" />
            <el-table-column prop="name" label="Name" width="180" />
            <el-table-column prop="address" label="Address" />
        </el-table>
    </pageMain>
</template>
  
<script lang="ts" setup>
const tableData = [
    {
        date: '2016-05-03',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-02',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-04',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
    {
        date: '2016-05-01',
        name: 'Tom',
        address: 'No. 189, Grove St, Los Angeles',
    },
]
let queryFormModel = ref({
    name: ''
})
const isFold = false;
</script>